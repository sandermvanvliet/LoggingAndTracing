const express = require('express')
const app = express()
const port = 3000

app.get('/api/users/:id', (req, res) => res.send('Hello User!'));
app.get('/api/cars/:id', (req, res) => res.send('Hello Car!'));

app.listen(port, () => console.log(`Mobile API listening on port ${port}!`))