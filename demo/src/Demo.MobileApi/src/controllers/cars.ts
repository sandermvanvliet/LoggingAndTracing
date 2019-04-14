import { Request, Response } from "express";
import { UrlOptions, CoreOptions } from "request";
import bunyan = require("bunyan");
import { get } from "express-http-context";

const defaultOptions = () => {
  return {
    url: process.env.CAR_API_URL,
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      "Demo-CorrelationId": get("correlation_id")
    }
  };
};

export const getCarById = (req: Request, res: Response) => {
  let logger = <bunyan>get("logger");

  let options = defaultOptions();
  options.url = `${options.url}/api/cars/${req.params.id}`;

  let request = require("request");

  request(<UrlOptions & CoreOptions>options, (error: any, response: Response, body: any) => {
    if(error != null) {
      logger.error(error);
    }
    else {
      res.status(response.statusCode);
      res.setHeader('Content-Type', 'application/json');
      res.send(body);
    }
  });
};
