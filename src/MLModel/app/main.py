from fastapi import FastAPI
from pydantic import BaseModel
from app.model.model import predict_pipeline
from app.model.model import __version__ as model_version


app = FastAPI()


class PicksIn(BaseModel):
    radiant_picked: str
    dire_picked: str
    banned: str


class PredictionOut(BaseModel):
    best_choice: str


@app.get("/")
def home():
    return {"health_check": "OK", "model_version": model_version}


@app.post("/predict", response_model=PredictionOut)
def predict(payload: PicksIn):
    best_choice = predict_pipeline(payload)
    return {"best_choice": best_choice}