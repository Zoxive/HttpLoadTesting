import Statistics from "../models/statistics";

export type HttpStatusResultStatisticsState =
{
    methods?: string[],
    requestUrls?: string[],
    statistics?: Statistics;
};

export default HttpStatusResultStatisticsState;