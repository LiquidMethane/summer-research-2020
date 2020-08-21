const mongoose = require('mongoose');
const Schema = mongoose.Schema;

const maintRecordSchema = new Schema({
    facility_id: {
        type: String,
        required: true,
    },

    timestamp: {
        type: Date,
        required: true,
    },

    technician: {
        type: String,
        required: true,
    },

    remarks: {
        type: String,
        required: true,
        default: 'N/A',
    }
});

module.exports = mongoose.model('MaintRecord', maintRecordSchema);