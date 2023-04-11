import pickle
import re
from pathlib import Path

__version__ = "0.1.0"

BASE_DIR = Path(__file__).resolve(strict=True).parent


with open(f"{BASE_DIR}/model-{__version__}.pkl", "rb") as f:
    model = pickle.load(f)


def predict_pipeline(input):

    radiantHeroes = list(map(int, input.radiant_picked.split(',')))
    direHeroes = list(map(int, input.dire_picked.split(',')))
    bannedHeroes = list(map(int, input.banned.split(',')))

    print(radiantHeroes)
    print(direHeroes)
    print(bannedHeroes)

    allHeroes = [i for i in range(139)]
    missedIdsInHeroList = [10,24,115,116,118,122,124,125,127,130,131,132,133,134]
    for el in missedIdsInHeroList:
        allHeroes.remove(el)

    print(allHeroes)

    for hero in radiantHeroes:
        allHeroes.remove(hero)
    for hero in direHeroes:
        print(hero)

        allHeroes.remove(hero)
    for hero in bannedHeroes:
        allHeroes.remove(hero)

    bestChoice = 0
    bestWinPercentage = 0
    for hero in allHeroes:
        if len(radiantHeroes) < len(direHeroes):
            radiantHeroes.append(hero)
            sorted(radiantHeroes)
            modelPrediction = model.predict_proba([radiantHeroes + direHeroes])[0, 0]
            if modelPrediction > bestWinPercentage:
                bestChoice = hero
                bestWinPercentage = modelPrediction
            radiantHeroes.remove(hero)
        elif len(direHeroes) < len(radiantHeroes):
            direHeroes.append(hero)
            sorted(direHeroes)
            modelPrediction = model.predict_proba([radiantHeroes + direHeroes])[0, 1]
            if modelPrediction > bestWinPercentage:
                bestChoice = hero
                bestWinPercentage = modelPrediction
            direHeroes.remove(hero)
        else:
            modelPredict = model.predict_proba([radiantHeroes + direHeroes])
            return f"Radiant percentage = {modelPredict[0, 0]}, Dire win percentage = {modelPredict[0, 1]}"

    return f"Best choice = {bestChoice}, Win percentage = {bestWinPercentage}"