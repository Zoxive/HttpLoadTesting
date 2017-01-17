export function createQueryString(parameters: any) : string
{
    var queryString = "";

    if(parameters.length === 0)
    {
        return queryString;
    }

    for(let key in parameters)
    {
        if(!parameters[key])
        {
            continue;
        }
        
        if(!queryString)
        {
            queryString = "?";
        }
        else
        {
            queryString += "&"
        }

        queryString += key + "=" + encodeURIComponent(parameters[key]);
    }

    return queryString;
}