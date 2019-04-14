import { Request, Response } from "express";
import { Guid } from "./guid";
import bunyan = require("bunyan");
import { NextFunction } from "connect";

export const correlationmiddleware = (req: Request, res: Response, next: NextFunction) => {
    let correlationId = req.headers["Demo-CorrelationId"];
    
    if(correlationId == null || correlationId == "") {
      correlationId = Guid.newGuid();
    }

    let logger = <bunyan>req.app.get("logger");

    req.app.set("logger", logger.child({correlation_id: correlationId}));
    
    next();
  };