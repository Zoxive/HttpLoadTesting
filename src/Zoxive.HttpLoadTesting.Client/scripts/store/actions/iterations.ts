import { Dispatch } from "redux";
import State from "../../store/state";

export const RECIEVE_ALL_ITERATIONS = "RECIEVE_ALL_ITERATIONS";

export interface IActionReceiveAllIterations extends IAction
{
}

export function fetchAllIterations()
{
    return (dispatch: Dispatch<State>, getState: () => State) =>
    {
        return fetch(`/api/all/`)
            .then(response => response.json())
            .then(json => dispatch(recieveAllIterations(json)));
    }
}

function recieveAllIterations(json: any): IActionReceiveAllIterations
{
    return {
        type: RECIEVE_ALL_ITERATIONS
    };
}