import { combineReducers, Dispatch } from "redux";
import { IActionGetStatistics } from "./statisticsAction"
import { IActionGetRequestMethods } from "./getRequestMethodsAction"
import { IActionGetRequestUrls } from "./getRequestUrlsAction"
import * as xtend from "xtend";

import { RECEIVE_STATISTICS } from "./statisticsAction";
import { RECEIVE_REQUEST_METHODS } from "./getRequestMethodsAction";
import { RECEIVE_REQUEST_URLS } from "./getRequestUrlsAction";

export default function reducer(state: any, action: IAction): any
{
    if (state == null)
    {
        var statistics = {
            method: "",
            requestUrl: "",
            numberOfStandardDeviations: 3,
            averageDuration: 0,
            durationCount: 0,
            standardDeviation: 0,
            averageDurationWithinStdDevs: 0,
            durationWithinStdDevsCount: 0
        }
        state = {
            methods: [],
            requestUrls: [],
            statistics: statistics
        };
    }

    switch (action.type)
    {
        case RECEIVE_STATISTICS:
            var statsAction = <IActionGetStatistics>action;
            return xtend(state, { statistics: statsAction.statistics });

        case RECEIVE_REQUEST_METHODS:
            var methodsAction = <IActionGetRequestMethods>action;
            return xtend(state, { methods: methodsAction.methods });

        case RECEIVE_REQUEST_URLS:
            var requestUrlsAction = <IActionGetRequestUrls>action;
            return xtend(state, { requestUrls: requestUrlsAction.requestUrls });
    }

    return state;
}