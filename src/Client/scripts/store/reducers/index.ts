import { combineReducers } from "redux";

import { IAction } from "~redux-thunk~redux";

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
        //
    }

    return state;
}