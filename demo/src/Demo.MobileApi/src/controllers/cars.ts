import { Request, Response } from "express";
import { UrlOptions, CoreOptions } from "request";

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
  let options = defaultOptions();

  options.url = `${options.url}/api/cars/${req.params.id}`;

  let request = require("request");

  request(<UrlOptions & CoreOptions>options, (error: any, response: Response, body: any) => {
    if(error != null) {
      console.log(error);
    }
    else {
      res.status(response.statusCode);
      res.setHeader('Content-Type', 'application/json');
      res.send(body);
    }
  });
};
