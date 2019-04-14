import { Request, Response } from "express";
import { Guid } from "./guid";
import bunyan = require("bunyan");
import { NextFunction } from "connect";
import { set, get } from "express-http-context";

export const correlationmiddleware = (req: Request, res: Response, next: NextFunction) => {
    let correlationId = req.get("Demo-CorrelationId");
    
    if(correlationId == null || correlationId == "") {
      correlationId = Guid.newGuid();
    }

    let logger = <bunyan>get("logger");

    set("logger", logger.child({correlation_id: correlationId}));
    
    next();
  };