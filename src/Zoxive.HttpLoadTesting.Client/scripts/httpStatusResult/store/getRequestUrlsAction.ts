import { Dispatch } from "redux";
import State from "../../store/state";

import { createQueryString } from "./../utils/urlUtils";

export const RECEIVE_REQUEST_URLS = "RECEIVE_REQUEST_URLS";

export interface IActionGetRequestUrls extends IAction
{
    requestUrls: string[]
}

export function fetchRequestUrls(method: string)
{
    var queryString = createQueryString({method});

    return (dispatch: Dispatch<State>, getState: () => State) =>
    {
        var url = `/api/httpStatusResult/requestUrls` + queryString;
        return fetch(url)
            .then(response => response.json())
            .then(json => dispatch(receiveRequestUrls(json)));
    };
}

function receiveRequestUrls(json: any): IActionGetRequestUrls
{
    return {
        type: RECEIVE_REQUEST_URLS,
        requestUrls: json
    };
}