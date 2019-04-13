import express from "express";
import * as bodyParser from "body-parser";
import * as usersController from "./controllers/users";
import * as carsController from "./controllers/cars";

const app = express();
const port = 3000;

app.use(bodyParser.json());
app.set("port", process.env.PORT || 3000);

app.get('/api/users/:id', (req, res) => usersController.getUserById(req, res));
app.get('/api/cars/:id', (req, res) => carsController.getCarById(req, res))

app.listen(port, () => console.log(`Mobile API listening on port ${port}!`));