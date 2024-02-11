let v = {
    block: "block",
    none: "none",
    showtitle: "showtitle",
    hidetitle: "hidetitle",
    volatilityBtn: "volatilityBtn",
    volatilityDiv: "volatilityDiv"
};

let gerfin = async () => {
    if (typeof Fingerprint2 === "undefined") return;

    await setTimeout(Fingerprint2.getV18({}, async (result, components) => {
        var st_diresu = window.localStorage.getItem('di_resu');
        var resu = st_diresu ? JSON.parse(st_diresu) : {};
        if (!resu[result]) {
            resu[result] = { c: 0, i: new Date() };
        } else {
            resu[result].c++;
        }
        resu[result].d = new Date();
        window.localStorage.setItem('di_resu', JSON.stringify(resu));

        await fetchData('/Data/GerFinDi', resu);
    }), 500);
}

let onstart = () => {
    if (window.requestIdleCallback) requestIdleCallback(() => { gerfin(); });
    else gerfin();
}

let chart = {};
let volatility = {};

var storedRange = null;

let getParam = (id) => document.getElementById(id).checked ? id : null;
let getDisplay = (id) => document.getElementById(id).style.display;
let setParam = (id, val) => document.getElementById(id).checked = val;
let setDisplay = (id, val) => document.getElementById(id).style.display = val;

let showVolatility = (value) => {
    let previousVal = document.getElementById(v.volatilityDiv).style.display;
    let newVal = previousVal === v.block ? v.none : v.block;

    if (typeof value !== "undefined") {
        newVal = value;
        previousVal = newVal === v.block ? v.none : v.block;
    }

    let btnTitle = document.getElementById(v.volatilityBtn).getAttribute(v.showtitle);

    if (previousVal === v.block) {
        newVal = v.none;
        btnTitle = document.getElementById(v.volatilityBtn).getAttribute(v.showtitle);
    }

    if (previousVal === v.none) {
        newVal = v.block;
        btnTitle = document.getElementById(v.volatilityBtn).getAttribute(v.hidetitle);
    }

    document.getElementById(v.volatilityBtn).value = btnTitle;
    document.getElementById(v.volatilityDiv).style.display = newVal;

    getControlSet();
};

let setSelectedElement = (id, valueToSelect) => {
    let element = document.getElementById(id);
    element.value = valueToSelect;
};

let restoreSettings = (settings) => {
    setSelectedElement('selCurrencies', settings['Currencies'][0]);
    setSelectedElement('selRange', settings['Range']);
    for (let operation in settings['Operations']) {
        document.getElementById(settings['Operations'][operation]).checked = true;
    }
    showVolatility(settings.Volatility);
};

function pArray(params) {
    let r = [];
    for (let p in params) {
        r.push(getParam(params[p]));
    }
    return r.filter(p => !!p);
}

function getParams(range) {
    storedRange = range || storedRange;

    let result = {
        Currencies: pArray(["USD", "EUR", "GBP", "CNY"]),
        Operations: pArray(["BUY", "SELL", "DIFF"]),
        Range: range || storedRange || 'W',
        Volatility: getDisplay(v.volatilityDiv)
    };

    window.localStorage.setItem('smarap', JSON.stringify(result));
    return result;
}

// TODO: remake to GET
var fetchData = (url, body) => {
    body = body || {};
    return fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    })
        .then(r => r.json());
}

var getChartData = async (params) => {
    var request = params || getControlSet();
    return await fetchData('/Data/GetData', {
        Currencies: request.Currencies,
        Operations: request.Operations,
        Range: request.Range
    });
}

var getVolatilityData = async () => {
    var data = await fetchData('/Data/GetVolatility');

    data.labels = JSON.parse(data.labels);
    data.percents = JSON.parse(data.percents);
    data.datasets = JSON.parse(data.datasets);

    return data;
}

var updateGauge = (val) => {
    setTimeout(() => {
        document.gauges.forEach((gauge) => gauge.value = val);
    }, 500);
}

var update = async (item) => {
    var needToUpdated = validateControlSet(item);
    if (!needToUpdated) return;

    chart.data = await getChartData();
    chart.update();
}

var getControlSet = () => {
    var result = {
        Currencies: [document.getElementById('selCurrencies').value],
        Operations: pArray(["BUY", "SELL", "DIFF"]),
        Range: document.getElementById('selRange').value,
        Volatility: getDisplay(v.volatilityDiv)
    }

    window.localStorage.setItem('smarap', JSON.stringify(result));

    return result;
}

var validateControlSet = (item) => {
    if (item === "DIFF") {
        setParam("BUY", false);
        setParam("SELL", false);
    }

    if (item === "BUY" || item === "SELL") {
        setParam("DIFF", false);
    }

    if (!getParam("BUY") && !getParam("SELL") && !getParam("DIFF")) {
        setParam(item, true);
        return false;
    }

    return true;
}

var createChart = (chartId, data, showLegends) =>
    Chart.Line(document.getElementById(chartId), {
        data: data,
        options: {
            legend: {
                display: showLegends,
                position: 'bottom',
                //position: 'top',
                align: 'start',
                fullWidth: true,
                labels: { filter: (label) => label.text !== null }
            },

            responsive: true,
            maintainAspectRatio: false,
            hoverMode: 'index',
            stacked: false,

            title: {
                display: true,
                text: data.title
            },

            scales: {
                yAxes: [{
                    type: 'linear',
                    display: true,
                    position: 'right',
                    id: 'y-axis-1'
                }]
            }
        }
    });

var initGauge = (item) => {
    var gauge = new RadialGauge({
        renderTo: 'canvas-' + item.id,
        width: 260,
        height: 260,
        units: "",
        title: item.title,

        startAngle: 90,
        ticksAngle: 180,
        colorPlate: "#ffffff",
        //colorPlateEnd: "#ffffff",
        colorUnits: "#3CA7DB",
        colorNumbers: "#534638",
        //colorNumbers: "#534638",
        colorNeedle: "#8E7860",
        colorNeedleEnd: "#8E7860",
        colorNeedleCircleOuter: "#8E7860",
        colorNeedleCircleOuterEnd: "#8E7860",

        colorNeedleShadowUp: "#8E7860",
        colorNeedleShadowDown: "#8E7860",

        colorMajorTicks: ["#EBEBEB", "#EBEBEB", "#EBEBEB", "#EBEBEB", "#EBEBEB", "#EBEBEB"],
        colorMinorTicks: "#EBEBEB",

        minValue: 0,
        maxValue: 100,
        majorTicks: ["", "20", "40", "60", "80", ""],
        minorTicks: "4",
        //minorTicks: "2",
        strokeTicks: true,
        highlights: [
            {
                "from": -0.25,
                "to": 30.00,
                "color": "#B1B9CB"
            },
            {
                "from": 30.00,
                "to": 70.00,
                "color": "#8FB9BD"
            },
            {
                "from": 70.00,
                "to": 100.25,
                "color": "#FF9488"
            }
        ],
        //
        highlightsWidth: 25,
        numbersMargin: 12,
        //
        //barWidth: 20,
        //barStrokeWidth: 0,
        //barProgress: 1,
        //barShadow: 0,
        //
        animation: true,
        //animationDuration: 500,
        animationRule: "linear",
        animatedValue: true,
        //animateOnInit: true,

        borders: false,
        valueBox: false,

        needleType: "arrow",
        needleEnd: 65,
        needleWidth: 4,
        needleCircleSize: 10,
        needleCircleInner: false,
        needleCircleOuter: true,
        needleShadow: false,

        borderShadowWidth: 0
    }).draw();

    gauge.value = item.value;
    gauge.id = item.id;
};

let detectMobile = () => (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent));

async function init() {
    setDisplay("panicMeters", "none");
    setDisplay("buttons", "none");

    let mobileMode = detectMobile();
    let params = JSON.parse(window.localStorage.getItem("smarap")) ||
    {
        Currencies: ["USD"],
        Operations: ["BUY"],
        Range: "W",
        Volatility: v.none
    };

    restoreSettings(params);

    var chartData = await getChartData(params);
    chart = createChart("canvas", chartData, !mobileMode);
    var volatilityData = await getVolatilityData();
    volatility = createChart("volatility", volatilityData, true);

    setDisplay("buttons", "block");

    if (!mobileMode) {
        volatilityData.percents.forEach((g) => initGauge(g));
        setDisplay("panicMeters", "block");
    }
}

init();

Array.prototype.forEach = Array.prototype.forEach || function (cb) {
    let i = 0, s = this.length;
    for (; i < s; i++) {
        cb && cb(this[i], i, this);
    }
}