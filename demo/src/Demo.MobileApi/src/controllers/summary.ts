import { Request, Response } from "express";
import { UrlOptions, CoreOptions } from "request";

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

export const get = (req: Request, res: Response) => {
  let carOptions = defaultCarOptions();
  carOptions.url = `${carOptions.url}/api/cars/VINNY_JONES`;

  let userOptions = defaultUserOptions();
  userOptions.url = `${userOptions.url}/api/users/MR_USER`;

  let request = require("request");

  request(<UrlOptions & CoreOptions>carOptions, (error: any, response: Response, body: any) => {
      if (error != null) {
        console.log(error);
        res.status(502);
        res.send();
      } else {
        let car = JSON.parse(body);

        request(<UrlOptions & CoreOptions>userOptions, (error: any, response: Response, body: any) => {
            if (error != null) {
              console.log(error);
              res.status(502);
              res.send();
            } else {
              let user = JSON.parse(body);
              let combined = { ...car, ...user };

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
