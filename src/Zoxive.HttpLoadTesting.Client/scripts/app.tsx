import * as React from "react";
import { render } from "react-dom";
import { Provider } from "react-redux";
import configureStore from "./store/configure";
import { fetchAllIterations } from "./store/actions/iterations";

var store = configureStore({});

/*
function Load()
{
    window.fetch("/api/all")
        .then(r => r.json())
        .then(json =>
        {
            store.dispatch(loadedAll(json));
        });
}
*/

var LoadAll = () => store.dispatch(fetchAllIterations());

LoadAll();

function AppHome()
{
    return (
        <div>
            <a onClick={LoadAll}>Load</a>
            <div>hi from react</div>
        </div>
    );
}

render(
    <Provider store={store}>
        <AppHome />
    </Provider>
, document.getElementById("react"));