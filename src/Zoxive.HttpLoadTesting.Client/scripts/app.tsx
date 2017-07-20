import * as React from "react";
import { render } from "react-dom";
import { Provider } from "react-redux";
import { BrowserRouter as Router, Route } from "react-router-dom";
import configureStore from "./store/configure";
import { fetchAllIterations } from "./store/actions/iterations";
import { fetchStatistics } from "./httpStatusResult/store/statisticsAction";
import { fetchMethods } from "./httpStatusResult/store/getRequestMethodsAction";
import { fetchRequestUrls } from "./httpStatusResult/store/getRequestUrlsAction";
import HttpStatusResultStatistics from "./httpStatusResult/components/httpStatusResultStatistics";

var store = configureStore({});

var LoadAll = () =>
{
    store.dispatch(fetchAllIterations());
    store.dispatch(fetchStatistics("", "", 3));
    store.dispatch(fetchMethods(""));
    store.dispatch(fetchRequestUrls(""));
};

LoadAll();

function AppHome()
{
    return (
        <div>
            <a onClick={LoadAll}>Click to Load All Iterations</a>
            <br/>
            <br/>
            <HttpStatusResultStatistics/>
        </div>
    );
}

render(
    <Provider store={store}>
        <Router>
            <AppHome />
        </Router>
    </Provider>
, document.getElementById("react"));