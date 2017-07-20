import { combineReducers } from "redux";
import { routerReducer } from "react-router-redux";
import State from "./state";
import indexReducer from "./reducers/index";
import statisticsReducer from "./../httpStatusResult/store/statisticsReducer";
import graphStatsReducer from "./../linegraph/store/graphStatsReducer";

const rootReducer = combineReducers<State>(
{
    index: indexReducer,
    statistics: statisticsReducer,
    graphStats: graphStatsReducer
});

export default rootReducer;