var express = require('express');
var app = express();

var sqlite3 = require('sqlite3').verbose();
var db = new sqlite3.Database("mainDB");

const fs = require('fs');
var counter = 0;

const bodyParser = require('body-parser');
app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
//Настройки
var port = 8812;
var validToken = "xxx";
var webSubdomain = "/telemetry";
//
//Обработчик пришедших сообщений (port 80, POST, application/json, "http://127.0.0.1/")
//
app.post(webSubdomain, function (req, res) {
	// console.log(req.body);
	if(req.body.message === "ping")
	{
		console.log(req.body.message);
		res.send('pong');
	}
	if(req.body.message === "checkToken")
	{
		var signature = JSON.parse(req.body.signature);
		if(signature.installationToken === validToken)
		{
			res.send("valid");
			console.log("valid token");
		}
		else
		{
			res.send("invalid");
			console.log("invalid token");
		}
	}
	if(req.body.message != "ping" && req.body.message != "checkToken")
	{
		console.log(req.body.message);
		res.send("thx");
	}
	
	console.log("\n-----------------------------------------------------------------------------------------------------------------------------\n");
	++counter;
});
app.listen(port, function () {
  console.log('Server is listening port ' + port);
});
var timeStart = new Date();
//
//Завершение работы
//
process.on("SIGINT", function(){
	console.log("\n");
	db.close();
	var timeEnd = new Date();
	var log = "[Server time start]: " + timeStart.toLocaleString() + "\n[Server shutdown time]: " + timeEnd.toLocaleString() + "\n[Quantity of request]: " + counter;
	console.log(log);
	fs.writeFileSync("log.txt", log);
	process.exit();
});