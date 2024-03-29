import express from "express";
import * as bodyParser from "body-parser";
import * as usersController from "./controllers/users";
import * as carsController from "./controllers/cars";
import * as summaryController from "./controllers/summary";
import bunyan from "bunyan";
import seq from "bunyan-seq";
import { Guid } from "./guid";
import { correlationmiddleware } from "./correlationmiddleware";
import { set, middleware } from "express-http-context";

const app = express();

// Configuration
app.use(bodyParser.json());
app.use(middleware); // Enable http context
app.set("port", process.env.PORT || 3000);

// Logging
var logger = bunyan.createLogger({
  name: 'mobileapi',
  streams: [
      seq.createStream({
          serverUrl: process.env.SEQ_URL || 'http://localhost:5341',
          level: 'info'
      })
  ],
  app_name: process.env.APP_NAME || 'unknown',
  app_instance: process.env.APP_INSTANCE || 'unknown'
});

app.use((req, res, next) => {
  set("logger", logger);
  next();
});

// Middleware
app.use(correlationmiddleware);

// Routing
app.get("/loadbalance/hello", (req, res) => {res.status(200); res.send(); });
app.get("/api/users/:id", (req, res) => usersController.getUserById(req, res));
app.get("/api/cars/:id", (req, res) => carsController.getCarById(req, res));
app.get("/api/summary", (req, res) => summaryController.getSummary(req, res));

// Startup
app.listen(app.get("port"), () =>
  logger.info(`App is running at http://localhost:${app.get("port")} in ${app.get("env")} mode`)
);