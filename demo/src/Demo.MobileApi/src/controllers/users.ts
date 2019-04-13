import { Request, Response } from "express";

export const getUserById = (req: Request, res: Response) => {
  res.status(200);
  res.send("{\"message:\": \"Hello user " + req.params.id + "!\"}");
};
