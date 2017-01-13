import Statistics from "../models/statistics";

export type HttpStatusResultStatisticsState =
{
    methods?: string[],
    requestUrls?: string[],
    numberOfDeviations?: number,
    statistics?: Statistics;
};

export default HttpStatusResultStatisticsState;