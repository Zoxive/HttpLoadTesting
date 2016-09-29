import { IAction, IActionGeneric, IDispatch } from "~redux-thunk~redux";

export const FETCH_ALL_ITERATIONS = "FETCH_ALL_ITERATIONS";

export interface IActionFetchAllIterations extends IAction
{
}

export function fetchAllIterations()
{
    return (dispatchEvent: IDispatch) =>
    {
    }
}

/*
export function fetchAllIterations(): IActionFetchAllIterations
{
    return {
        type: FETCH_ALL_ITERATIONS
    }
}
*/