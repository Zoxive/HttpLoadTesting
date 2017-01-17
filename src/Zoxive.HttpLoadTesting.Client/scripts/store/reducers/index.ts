import { combineReducers, Dispatch } from "redux";
import State from "../../store/state";

import { RECIEVE_ALL_ITERATIONS } from "../actions/iterations";

export default function reducer(state: any, action: IAction): any
{
    if (state == null)
    {
        state = {
            iteration: {}
        };
    }

    switch (action.type)
    {
        case RECIEVE_ALL_ITERATIONS:
            //console.log(action);
            break;
    }

    return state;
}