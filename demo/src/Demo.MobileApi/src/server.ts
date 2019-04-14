import express from "express";
import * as bodyParser from "body-parser";
import * as usersController from "./controllers/users";
import * as carsController from "./controllers/cars";
import * as summaryController from "./controllers/summary";

const app = express();

app.use(bodyParser.json());
app.set("port", process.env.PORT || 3000);

app.get("/loadbalance/hello", (req, res) => {res.status(200); res.send(); });

app.get("/api/users/:id", (req, res) => usersController.getUserById(req, res));
app.get("/api/cars/:id", (req, res) => carsController.getCarById(req, res));
app.get("/api/summary", (req, res) => summaryController.get(req, res));

app.listen(app.get("port"), () =>
  console.log(
    "  App is running at http://localhost:%d in %s mode",
    app.get("port"),
    app.get("env")
  )
);