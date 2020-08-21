const mongoose = require('mongoose');
const Schema = mongoose.Schema;

const liveFeedSchema = new Schema({
    facility_id: {
        type: String,
        required: true,
    },

    timestamp: {
        type: Date,
        required: true,
    },

    wattage: {
        type: Number,
        required: true,
    },

    temp: {
        type: Number,
        required: true,
    }
});

module.exports = mongoose.model('LiveFeed', liveFeedSchema);