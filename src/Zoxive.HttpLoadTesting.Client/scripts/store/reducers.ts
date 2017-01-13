import { combineReducers } from "redux";
import { routerReducer } from "react-router-redux";
import State from "./state";
import indexReducer from "./reducers/index";
import statisticsReducer from "./../httpStatusResult/store/statisticsReducer";

const rootReducer = combineReducers<State>(
{
    index: indexReducer,
    statistics: statisticsReducer
});

export default rootReducer;