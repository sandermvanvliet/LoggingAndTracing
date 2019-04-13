"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getUserById = function (req, res) {
    res.status(200);
    res.send("Hello user" + req.params.id + "!");
};
