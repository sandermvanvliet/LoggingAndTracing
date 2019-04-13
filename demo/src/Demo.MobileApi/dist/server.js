"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (Object.hasOwnProperty.call(mod, k)) result[k] = mod[k];
    result["default"] = mod;
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
var express_1 = __importDefault(require("express"));
var bodyParser = __importStar(require("body-parser"));
var usersController = __importStar(require("./controllers/users"));
var carsController = __importStar(require("./controllers/cars"));
var app = express_1.default();
var port = 3000;
app.use(bodyParser.json());
app.set("port", process.env.PORT || 3000);
app.get('/api/users/:id', function (req, res) { return usersController.getUserById(req, res); });
app.get('/api/cars/:id', function (req, res) { return carsController.getCarById(req, res); });
app.listen(port, function () { return console.log("Mobile API listening on port " + port + "!"); });
