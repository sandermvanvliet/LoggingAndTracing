import { Request, Response } from "express";

export const getCarById = (req: Request, res: Response) => {
  res.status(200);
  res.send("Hello car " + req.params.id + "!");
};