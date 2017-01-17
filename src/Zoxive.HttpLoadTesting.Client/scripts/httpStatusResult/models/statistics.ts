export type Statistics =
{
    method: string;
    requestUrl: string;
    numberOfStandardDeviations: number;
    averageDuration: number;
    durationCount: number;
    standardDeviation: number;
    averageDurationWithinDeviations: number;
    durationWithinDeviationsCount: number;
    fastestRequests: any[];
    slowestRequests: any[];
}

export default Statistics;