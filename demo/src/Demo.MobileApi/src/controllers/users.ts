import { Request, Response } from "express";
import { RequestCallback, UrlOptions, CoreOptions } from "request";

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
  let options = defaultOptions();

  options.url = `${options.url}/api/users/${req.params.id}`;

  let request = require("request");

  console.log(`Calling ${options.url}`);

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
