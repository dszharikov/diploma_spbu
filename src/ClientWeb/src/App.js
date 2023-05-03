import React, { useState } from 'react';
import { Button, Form, Container, Col, Row, Alert } from 'react-bootstrap'
import 'bootstrap/dist/css/bootstrap.min.css'

function isListOfNumbers(str) {
  const arr = str.split(',');

  const isAllNumbers = arr.every(elem => !isNaN(elem));

  return isAllNumbers;
}

function isInputCorrect(radiant, dire, banned, allHeroes) {
  if (isListOfNumbers(radiant) && isListOfNumbers(dire) && isListOfNumbers(banned)) {
    let radiantHeroes = radiant.split(',');
    let direHeroes = dire.split(',');
    let bannedHeroes = banned.split(',');

    if (radiantHeroes.length == 5 && (direHeroes.length == 4 || direHeroes.length == 5)
      || radiantHeroes.length == 4 && direHeroes == 5) {
      let currentHeroes = radiantHeroes.concat(direHeroes);
      currentHeroes = currentHeroes.concat(bannedHeroes);

      console.log(allHeroes.length);

      console.log(currentHeroes.length);


      // проверить на повторки и на наличие всех ребят в allHeroes
      const listOfHeroes = [];
      currentHeroes.forEach(index => {
        const id = parseInt(index);

        console.log(index + " " + id);

        if (allHeroes.includes(id)) {
          console.log("allHeroes includes id " + id);
          if (!listOfHeroes.includes(id)) {
            listOfHeroes.push(id);
          } else {
            // hero ids are repeated
            console.log("hero ids are repeated id: " + id);

            return false;
          }
        } else {
          console.log("hero id is incorrect id: " + id);
          // hero id is incorrect
          return false;
        }
      });

      return true;
    } else {
      return false;
    }
  }
  else
    return false;
}

function App() {
  const [formData, setFormData] = useState({
    radiant: '',
    dire: '',
    banned: ''
  });

  const [response, setResponse] = useState(null);

  let allHeroes = [];
  for (let i = 0; i < 139; i++) {
    allHeroes.push(i);
  }
  let missedIdsInHeroList = [10, 24, 115, 116, 118, 122, 124, 125, 127, 130, 131, 132, 133, 134];
  for (let i = 0; i < missedIdsInHeroList.length; i++) {
    let index = allHeroes.indexOf(missedIdsInHeroList[i]);
    if (index !== -1) {
      allHeroes.splice(index, 1);
    }
  }

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    setFormData({ ...formData, [name]: value });
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    if (isInputCorrect(formData.radiant, formData.dire, formData.banned, allHeroes)) {
      const json = JSON.stringify(formData);
      console.log(json);
      fetch('http://localhost:8080/predict', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: json
      })
        .then(response => response.json())
        .then(data => {
          setResponse(data);
        })
        .catch(error => {
          console.error('Error:', error);
        });
    }
    else
      setResponse("Input is incorrect");

  };

  return (
    <>
      <div className="App">
        <Container>
          <Row>
            <Col>
              <Form onSubmit={handleSubmit}>
                <Form.Group controlId="radiant" className="mb-3">
                  <Form.Label>Radiant Heroes:</Form.Label>
                  <Form.Control type="text" name="radiant" value={formData.radiant} onChange={handleInputChange} />
                  <Form.Text className="text-muted">
                  </Form.Text>
                </Form.Group>
                <Form.Group controlId="dire" className="mb-3">
                  <Form.Label>Dire Heroes:</Form.Label>
                  <Form.Control type="text" name="dire" value={formData.dire} onChange={handleInputChange} />
                </Form.Group>
                <Form.Group controlId="banned" className="mb-3">
                  <Form.Label>Banned Heroes:</Form.Label>
                  <Form.Control type="text" name="banned" as="textarea" rows={3} value={formData.banned} onChange={handleInputChange} />
                </Form.Group>
                <Button variant="primary" type="submit">
                  Submit
                </Button>
              </Form>
            </Col>
          </Row>
        </Container>
      </div>
      {response && (
        <Alert variant="success">
          <pre>{JSON.stringify(response, null, 2)}</pre>
        </Alert>
      )}
      <b>{allHeroes.length}</b>
    </>
  );
}

export default App;