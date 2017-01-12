import * as React from "react";
import { render } from "react-dom";
import { Provider } from "react-redux";
import configureStore from "./store/configure";
import { fetchAllIterations } from "./store/actions/iterations";
import HttpStatusResultStatistics from "./httpStatusResult/components/httpStatusResultStatistics";

var store = configureStore({});

var LoadAll = () => store.dispatch(fetchAllIterations());

LoadAll();

function AppHome()
{
    return (
        <div>
            <a onClick={LoadAll}>Load</a>
            <div>hi from react</div>
            <HttpStatusResultStatistics  />
        </div>
    );
}

render(
    <Provider store={store}>
        <AppHome />
    </Provider>
, document.getElementById("react"));