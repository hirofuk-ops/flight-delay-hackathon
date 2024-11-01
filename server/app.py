from flask import Flask, request, jsonify
import joblib
import csv
import pandas as pd

app = Flask(__name__)
# Enable cors
@app.after_request
def after_request(response):
    """
    Enable CORS
    """
    response.headers.add('Access-Control-Allow-Origin', '*')
    response.headers.add('Access-Control-Allow-Headers', 'Content-Type,Authorization')
    return response

# Load your pre-trained model
model = joblib.load('/workspaces/flight-delay-hackathon/data/model.pkl')

@app.route('/predict', methods=['GET'])
def predict_delay():
    """
    Takes two parameters - day of week and airport id, then returns a prediction of flight delay
    """
    # Store day_of_week as int
    day_of_week = int(request.args.get('day_of_week'))
    airport_id = int(request.args.get('airport_id'))
    prediction = model.predict_proba([[day_of_week, airport_id]])[0]
    
    # Split prediction string by space
    prediction = str(prediction).split(' ')

    # store first value from prediction as certainty, and remove the first character
    certainty = float(prediction[0][2:])

    # store second value from prediction as delay, and remove the last character
    delay = float(prediction[1][:-1])

    # return prediction as json
    return jsonify({'certainty': certainty, 'delay': delay})

@app.route('/airports', methods=['GET'])
def get_airports():
    airports = []
    with open('/workspaces/flight-delay-hackathon/data/airports.csv', newline='') as csvfile:
        reader = csv.DictReader(csvfile)
        for row in reader:
            airports.append({'id': row['OriginAirportID'], 'name': row['OriginAirportName']})
    
    # Sort airports by name
    airports.sort(key=lambda x: x['name'])
    
    return jsonify(airports)

if __name__ == '__main__':
    app.run()