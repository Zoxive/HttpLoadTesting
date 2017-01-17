import thunk from "redux-thunk";
import { createStore, applyMiddleware, compose, Store, Middleware } from "redux";
import reducer from "./reducers";
import * as createLogger from "redux-logger";
import State from "./state";

const middleware: Middleware[] = [thunk];

if (__DEV__)
{
    // logger middleware
    let logger = createLogger(
    {
        collapsed: true,
        level: 'info',
        duration: true
    });
    middleware.push(logger);
}

const finalCreateStore = compose(applyMiddleware.apply(applyMiddleware, middleware))(createStore);

export default function configureStore(state: any): Store<State>
{
    const store = finalCreateStore(reducer, state) as Store<State>;

    return store;
}