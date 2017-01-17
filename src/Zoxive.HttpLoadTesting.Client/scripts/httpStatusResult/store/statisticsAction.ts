import { Dispatch } from "redux";
import State from "../../store/state";
import Statistics from "../models/statistics";

import { createQueryString } from "./../utils/urlUtils";

export const RECEIVE_STATISTICS = "RECEIVE_STATISTICS";

export interface IActionGetStatistics extends IAction
{
    statistics: Statistics
}

export function fetchStatistics(method: string, requestUrl: string, deviations: number)
{
    var queryString = createQueryString({method, requestUrl, deviations});

    return (dispatch: Dispatch<State>, getState: () => State) =>
    {
        var url = `/api/httpStatusResult/statistics` + queryString;
        return fetch(url)
            .then(response => response.json())
            .then(json => dispatch(receiveStatistics(json)));
    };
}

function receiveStatistics(json: any): IActionGetStatistics
{
    return {
        type: RECEIVE_STATISTICS,
        statistics: json
    };
}