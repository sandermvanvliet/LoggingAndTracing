import { Request, Response } from "express";
import { RequestCallback, UrlOptions, CoreOptions } from "request";
import bunyan = require("bunyan");

const defaultOptions = () => {
  return {
    url: process.env.USER_API_URL,
    method: "GET",
    headers: {
      "Content-Type": "application/json"
    }
  };
};

export const getUserById = (req: Request, res: Response) => {
  let logger = <bunyan>req.app.get("logger");

  let options = defaultOptions();

  options.url = `${options.url}/api/users/${req.params.id}`;

  let request = require("request");

  logger.info(`Calling ${options.url}`);

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
