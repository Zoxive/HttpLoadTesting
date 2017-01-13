import { Dispatch } from "redux";
import State from "../../store/state";
import Statistics from "../models/statistics"

export const RECEIVE_STATISTICS = "RECEIVE_STATISTICS";

export interface IActionGetStatistics extends IAction
{
    statistics: Statistics
}

export function fetchStatistics()
{
    return (dispatch: Dispatch<State>, getState: () => State) =>
    {
        var url = `/api/httpStatusResult/statistics`;
        return fetch(url)
            .then(response => response.json())
            .then(json => dispatch(receiveStatistics(json)));
    }
}

function receiveStatistics(json: any): IActionGetStatistics
{
    return {
        type: RECEIVE_STATISTICS,
        statistics: json
    };
}