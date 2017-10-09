import * as React from "react";
import { render } from "react-dom";
import { Provider } from "react-redux";
import { BrowserRouter as Router, Route, Link } from "react-router-dom";
import configureStore from "./store/configure";
import HttpStatusResultStatistics from "./httpStatusResult/components/httpStatusResultStatistics";
import LineGraph from "./linegraph/linegraph";

var store = configureStore({});

render(
    <Provider store={store}>
        <Router>
            <div>
                <ul>
                    <li><Link to="/">Home</Link></li>
                    <li><Link to="/stats">Statistics</Link></li>
                    <li><Link to="/linegraph">Line Graph</Link></li>
                </ul>
                <Route exact path="/" component={() => <div><h1>Home</h1></div>}/>
                <Route path="/stats" component={HttpStatusResultStatistics} />
                <Route path="/linegraph" component={LineGraph} />
            </div>
        </Router>
    </Provider>
, document.getElementById("react"));