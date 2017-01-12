import thunk from "redux-thunk";
import { createStore, applyMiddleware, compose, Store } from "redux";
import reducer from "./reducers/index";
import State from "./state";

var middleware: any[] = [thunk];

if (__DEV__)
{
    // logger middleware
}

const finalCreateStore = compose(applyMiddleware.apply(applyMiddleware, middleware))(createStore);

export default function configureStore(state: any): Store<State>
{
    const store = finalCreateStore(reducer, state) as Store<State>;

    return store;
}