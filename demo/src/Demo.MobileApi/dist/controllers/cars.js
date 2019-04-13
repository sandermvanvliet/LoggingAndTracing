"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getCarById = function (req, res) {
    res.status(200);
    res.send("Hello car " + req.params.id + "!");
};
