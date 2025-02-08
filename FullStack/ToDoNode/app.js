const express = require('express');
const renderApi = require("@api/render-api");

const app = express();
const PORT = 3000;

renderApi.auth('rnd_SW8xHVe3MeAs1ztDMLVNHonwnC2C');

app.get('/', async (req, res) => {
    try {
        const { data } = await renderApi.listServices({
            includePreviews: 'true', limit: '20'
        })
        res.json(data);
    } catch (error) {
        res.status(500).send('Error fetching services');
    }
});

app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});
