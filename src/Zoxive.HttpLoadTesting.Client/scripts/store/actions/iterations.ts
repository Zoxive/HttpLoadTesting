import { IAction, IActionGeneric, IDispatch } from "~redux-thunk~redux";

export const RECIEVE_ALL_ITERATIONS = "RECIEVE_ALL_ITERATIONS";

export interface IActionReceiveAllIterations extends IAction
{
}

export function fetchAllIterations()
{
    return (dispatch: IDispatch) =>
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