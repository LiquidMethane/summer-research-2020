const mongoose = require('mongoose');
const Schema = mongoose.Schema;

const facilitySchema = new Schema({
    _id: {
        type: String,
        required: true,
    },
    
    name: {
        type: String,
        required: true,
    },

    type: {
        type: String,
        required: true,
    },

    dop: {
        type: Date,
        required: true,
    },

    status: {
        type: String,
        required: true,
        default: 'Stopped',
    }


});

module.exports = mongoose.model('Facility', facilitySchema);