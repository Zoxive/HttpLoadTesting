import { Dispatch } from "redux";
import State from "../../store/state";

export const RECEIVE_REQUEST_METHODS = "RECEIVE_REQUEST_METHODS";

export interface IActionGetRequestMethods extends IAction
{
    methods: string[]
}

export function fetchMethods(requestUrl: string)
{
    return (dispatch: Dispatch<State>, getState: () => State) =>
    {
        var url = `/api/httpStatusResult/methods`;
        return fetch(url)
            .then(response => response.json())
            .then(json => dispatch(receiveRequestMethods(json)));
    }
}

function receiveRequestMethods(json: any): IActionGetRequestMethods
{
    return {
        type: RECEIVE_REQUEST_METHODS,
        methods: json
    };
}