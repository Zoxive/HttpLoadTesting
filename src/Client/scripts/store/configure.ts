import thunk from "redux-thunk";
import { createStore, applyMiddleware, compose } from "redux";
import reducer from "./reducers/index";

var middleware: any[] = [thunk];

if (__DEV__)
{
    // logger middleware
}

const finalCreateStore = compose(applyMiddleware.apply(applyMiddleware, middleware))(createStore);

export default function configureStore(state: any): any
{
    const store = finalCreateStore(reducer, state);

    return store;
}