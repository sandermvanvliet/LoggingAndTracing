import { Request, Response } from "express";
import { UrlOptions, CoreOptions } from "request";
import bunyan = require("bunyan");
import { get } from "express-http-context";

const defaultCarOptions = () => {
  return {
    url: process.env.CAR_API_URL,
    method: "GET",
    headers: {
      "Content-Type": "application/json"
    }
  };
};
const defaultUserOptions = () => {
  return {
    url: process.env.USER_API_URL,
    method: "GET",
    headers: {
      "Content-Type": "application/json"
    }
  };
};

export const getSummary = (req: Request, res: Response) => {
  let carOptions = defaultCarOptions();
  carOptions.url = `${carOptions.url}/api/cars/VINNY_JONES`;

  let userOptions = defaultUserOptions();
  userOptions.url = `${userOptions.url}/api/users/MR_USER`;

  let request = require("request");

  let logger = <bunyan>get("logger");

  logger.info("Retrieving car information");
  request(<UrlOptions & CoreOptions>carOptions, (error: any, response: Response, body: any) => {
      if (error != null) {
        logger.error(error);
        res.status(502);
        res.send();
      } else {
        logger.info("Retrieving user information");
        let car = JSON.parse(body);

        request(<UrlOptions & CoreOptions>userOptions, (error: any, response: Response, body: any) => {
            if (error != null) {
              logger.error(error);
              res.status(502);
              res.send();
            } else {
              let user = JSON.parse(body);
              let combined = { ...car, ...user };
              
              logger.info("Created summary for car and user");

              res.status(response.statusCode);
              res.setHeader("Content-Type", "application/json");
              res.send(combined);
            }
          }
        );
      }
    }
  );
};
