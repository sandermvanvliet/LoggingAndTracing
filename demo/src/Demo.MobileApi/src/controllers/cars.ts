import { Request, Response } from "express";
import { UrlOptions, CoreOptions } from "request";
import bunyan = require("bunyan");

const defaultOptions = () => {
  return {
    url: process.env.CAR_API_URL,
    method: "GET",
    headers: {
      "Content-Type": "application/json"
    }
  };
};

export const getCarById = (req: Request, res: Response) => {
  let logger = <bunyan>req.app.get("logger");

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
