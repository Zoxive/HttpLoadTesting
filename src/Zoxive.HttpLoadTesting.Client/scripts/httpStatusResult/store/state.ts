import Statistics = require("../../models/statistics");

export type HttpStatusResultStatisticsState =
{
    method: string,
    requestUrl: string,
    numberOfStdDevs: number,
    statistics: Statistics.Statistics;
};

export default HttpStatusResultStatisticsState;