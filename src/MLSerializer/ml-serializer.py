from fastapi import FastAPI
import time
from pydantic import BaseModel
from pathlib import Path
import pickle
from peewee import *
import os
import pandas as pd
import numpy as np


__version__ = "0.1.0"

databaseConnection = os.environ.get('DB_CONNECTION')
pg_db = PostgresqlDatabase(databaseConnection)


class BaseModel(Model):
    class Meta:
        database = pg_db


class Match(BaseModel):
    match_id = BigAutoField(column_name='MatchId')
    radiant_win = BooleanField(column_name='RadiantWin')
    start_time = BigIntegerField(column_name='StartTime')

    duration = IntegerField(column_name='Duration')
    radiant_team = TextField(column_name='RadiantTeam', null=False)
    dire_team = TextField(column_name='DireTeam', null=False)
    average_mmr = IntegerField(column_name='AverageMMR', null=True)

    class Meta:
        table_name = 'Matches'


app = FastAPI()


@app.get("/")
def home():
    return {"health_check": "OK", "model_version": __version__}


@app.get("/refit")
def refit():
    unix_time_now = int(time.time())
    unix_time_yesterday = unix_time_now - 86400 # one unix timestamp day
    query = Match.select().where(Match.start_time > unix_time_yesterday).order_by(Match.match_id.desc())

    matches_selected = query.dicts().execute()

    df2 = pd.DataFrame([m.__dict__ for m in matches_selected ])

    BASE_DIR = Path(__file__).resolve(strict=True).parent

    with open(f"{BASE_DIR}/model-{__version__}.pkl", "rb") as f:
        model = pickle.load(f)

    logDf = df2.drop(columns=['RadiantTeam', 'DireTeam'])
    features = logDf.drop(columns=['RadiantWin'])
    labels = logDf['RadiantWin']
    logDf.head(5)

    features
    input_data = []
    for i, j in tqdm(features.iterrows()):
        arr1 = np.zeros(138*5)
        arr2 = np.zeros(138*5)
        for hero_ind in range(1, 6):
            arr1[138*(hero_ind-1) + int(j['RadiantHero%s' % hero_ind])] = 1
            arr2[138*(hero_ind-1) + int(j['DireHero%s' % hero_ind])] = 1
        concatenated_arr = np.concatenate([arr1, arr2])
        input_data += [concatenated_arr[:1500].astype(bool)]

    x = np.array(input_data)

    model.fit(x, labels, epoch=100)

    os.remove(f'{BASE_DIR}/model-{__version__}.pkl')
    pickle.dump(model, open(f'{BASE_DIR}/model-{__version__}.pkl', 'wb'))

    return {"Done!"}
