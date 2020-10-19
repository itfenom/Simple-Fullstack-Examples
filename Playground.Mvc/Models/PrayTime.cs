using System;

namespace Playground.Mvc.Models
{
    public class PrayTime
    {
        // Calculation Methods
        public static int Jafari = 0;    // Ithna Ashari

        public static int Karachi = 1;    // University of Islamic Sciences, Karachi
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once InconsistentNaming
        public static int ISNA = 2;    // Islamic Society of North America (ISNA)
        // ReSharper disable once InconsistentNaming
        public static int MWL = 3;    // Muslim World League (MWL)
        public static int Makkah = 4;    // Umm al-Qura, Makkah
        public static int Egypt = 5;    // Egyptian General Authority of Survey
        public static int Custom = 6;    // Custom Setting
        public static int Tehran = 7;    // Institute of Geophysics, University of Tehran

        // Juristic Methods
        public static int Shafii = 0;    // Shafii (standard)

        public static int Hanafi = 1;    // Hanafi

        // Adjusting Methods for Higher Latitudes
        public static int None = 0;    // No adjustment

        public static int MidNight = 1;    // middle of night
        public static int OneSeventh = 2;    // 1/7th of night
        public static int AngleBased = 3;    // angle/60th of night

        // Time Formats
        public static int Time24 = 0;    // 24-hour format

        public static int Time12 = 1;    // 12-hour format
        public static int Time12Ns = 2;    // 12-hour format with no suffix
        public static int Floating = 3;    // floating point number

        // Time Names
        //public static String[] TimeNames = { "Fajr", "Sunrise", "Dhuhr", "Asr", "Sunset", "Maghrib", "Isha" };

        private static String InvalidTime = "----";	 // The string used for inv

        //---------------------- Global Variables --------------------

        private int _calcMethod = 3;		// caculation method
        private int _asrJuristic;		// Juristic method for Asr
        // ReSharper disable once IdentifierTypo
        private int _dhuhrMinutes;		// minutes after mid-day for Dhuhr
        private int _adjustHighLats = 1;	// adjusting method for higher latitudes

        private int _timeFormat;		// time format

        private double _lat;        // latitude
        private double _lng;        // longitude
        private int _timeZone;   // time-zone
        private double _jDate;      // Julian date

        //--------------------- Technical Settings --------------------

        private int numIterations = 1;		// number of iterations needed to compute times

        //------------------- Calc Method Parameters --------------------

        private double[][] methodParams;

        public PrayTime()
        {
            methodParams = new double[8][];
            methodParams[Jafari] = new double[] { 16, 0, 4, 0, 14 };
            methodParams[Karachi] = new double[] { 18, 1, 0, 0, 18 };
            methodParams[ISNA] = new double[] { 15, 1, 0, 0, 15 };
            methodParams[MWL] = new double[] { 18, 1, 0, 0, 17 };
            methodParams[Makkah] = new[] { 18.5, 1, 0, 1, 90 };
            methodParams[Egypt] = new[] { 19.5, 1, 0, 0, 17.5 };
            methodParams[Tehran] = new[] { 17.7, 0, 4.5, 0, 14 };
            methodParams[Custom] = new double[] { 18, 1, 0, 0, 17 };
        }

        // return prayer times for a given date
        // ReSharper disable once UnusedMember.Global
        public string[] GetPrayerTimes(int year, int month, int day, double latitude, double longitude, int timeZone)
        {
            return GetDatePrayerTimes(year, month + 1, day, latitude, longitude, timeZone);
        }

        // set the calculation method
        public void SetCalcMethod(int methodId)
        {
            _calcMethod = methodId;
        }

        // set the juristic method for Asr
        public void SetAsrMethod(int methodId)
        {
            if (methodId < 0 || methodId > 1)
                return;

            _asrJuristic = methodId;
        }

        // set the angle for calculating Fajr
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetFajrAngle(double angle)
        {
            SetCustomParams(new[] { (int)angle, -1, -1, -1, -1 });
        }

        // set the angle for calculating Maghrib
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetMaghribAngle(double angle)
        {
            SetCustomParams(new[] { -1, 0, (int)angle, -1, -1 });
        }

        // set the angle for calculating Isha
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetIshaAngle(double angle)
        {
            SetCustomParams(new[] { -1, -1, -1, 0, (int)angle });
        }

        // set the minutes after mid-day for calculating Dhuhr
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetDhuhrMinutes(int minutes)
        {
            _dhuhrMinutes = minutes;
        }

        // set the minutes after Sunset for calculating Maghrib
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetMaghribMinutes(int minutes)
        {
            SetCustomParams(new[] { -1, 1, minutes, -1, -1 });
        }

        // set the minutes after Maghrib for calculating Isha
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetIshaMinutes(int minutes)
        {
            SetCustomParams(new[] { -1, -1, -1, 1, minutes });
        }

        // set custom values for calculation parameters
        public void SetCustomParams(int[] param)
        {
            for (int i = 0; i < 5; i++)
            {
                if (param[i] == -1)
                    methodParams[Custom][i] = methodParams[_calcMethod][i];
                else
                    methodParams[Custom][i] = param[i];
            }
            _calcMethod = Custom;
        }

        // set adjusting method for higher latitudes
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetHighLatsMethod(int methodId)
        {
            _adjustHighLats = methodId;
        }

        // set the time format
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public void SetTimeFormat(int timeFormat)
        {
            _timeFormat = timeFormat;
        }

        // convert float hours to 24h format
        public string FloatToTime24(double time)
        {
            if (time < 0)
                return InvalidTime;
            time = FixHour(time + 0.5 / 60);  // add 0.5 minutes to round
            double hours = Math.Floor(time);
            double minutes = Math.Floor((time - hours) * 60);
            return TwoDigitsFormat((int)hours) + ":" + TwoDigitsFormat((int)minutes);
        }

        // convert float hours to 12h format
        public string FloatToTime12(double time, bool noSuffix)
        {
            if (time < 0)
                return InvalidTime;
            time = FixHour(time + 0.5 / 60);  // add 0.5 minutes to round
            double hours = Math.Floor(time);
            double minutes = Math.Floor((time - hours) * 60);
            string suffix = hours >= 12 ? " pm" : " am";
            hours = (hours + 12 - 1) % 12 + 1;
            return ((int)hours) + ":" + TwoDigitsFormat((int)minutes) + (noSuffix ? "" : suffix);
        }

        // convert float hours to 12h format with no suffix
        public string FloatToTime12Ns(double time)
        {
            return FloatToTime12(time, true);
        }

        //---------------------- Compute Prayer Times -----------------------

        // return prayer times for a given date
        public string[] GetDatePrayerTimes(int year, int month, int day, double latitude, double longitude,

        int timeZone)
        {
            _lat = latitude;
            _lng = longitude;
            _timeZone = timeZone;
            _jDate = JulianDate(year, month, day) - longitude / (15 * 24);

            return ComputeDayTimes();
        }

        // compute declination angle of sun and equation of time
        public double[] SunPosition(double jd)
        {
            double dd = jd - 2451545.0;
            double g = FixAngle(357.529 + 0.98560028 * dd);
            double q = FixAngle(280.459 + 0.98564736 * dd);
            double l = FixAngle(q + 1.915 * Dsin(g) + 0.020 * Dsin(2 * g));

            //double r = 1.00014 - 0.01671 * dcos(g) - 0.00014 * dcos(2 * g);
            double e = 23.439 - 0.00000036 * dd;

            double d = Darcsin(Dsin(e) * Dsin(l));
            double ra = Darctan2(Dcos(e) * Dsin(l), Dcos(l)) / 15;
            ra = FixHour(ra);
            double eqT = q / 15 - ra;

            return new[] { d, eqT };
        }

        // compute equation of time
        public double EquationOfTime(double jd)
        {
            return SunPosition(jd)[1];
        }

        // compute declination angle of sun
        public double SunDeclination(double jd)
        {
            return SunPosition(jd)[0];
        }

        // compute mid-day (Dhuhr, Zawal) time
        public double ComputeMidDay(double t)
        {
            double T = EquationOfTime(_jDate + t);
            double z = FixHour(12 - T);
            return z;
        }

        // compute time for a given angle G
        public double ComputeTime(double g, double t)
        {
            //System.out.println("G: "+G);

            double d = SunDeclination(_jDate + t);
            double z = ComputeMidDay(t);
            double v = ((double)1 / 15) * Darccos((-Dsin(g) - Dsin(d) * Dsin(_lat)) /
                    (Dcos(d) * Dcos(_lat)));
            return z + (g > 90 ? -v : v);
        }

        // compute the time of Asr
        public double ComputeAsr(int step, double t)  // Shafii: step=1, Hanafi: step=2
        {
            double d = SunDeclination(_jDate + t);
            double g = -Darccot(step + Dtan(Math.Abs(_lat - d)));
            return ComputeTime(g, t);
        }

        //---------------------- Compute Prayer Times -----------------------

        // compute prayer times at given julian date
        public double[] ComputeTimes(double[] times)
        {
            double[] t = DayPortion(times);

            double fajr = ComputeTime(180 - methodParams[_calcMethod][0], t[0]);
            double sunrise = ComputeTime(180 - 0.833, t[1]);
            double dhuhr = ComputeMidDay(t[2]);
            double asr = ComputeAsr(1 + _asrJuristic, t[3]);
            double sunset = ComputeTime(0.833, t[4]); 
            double maghrib = ComputeTime(methodParams[_calcMethod][2], t[5]);
            double isha = ComputeTime(methodParams[_calcMethod][4], t[6]);

            return new[] { fajr, sunrise, dhuhr, asr, sunset, maghrib, isha };
        }

        // adjust Fajr, Isha and Maghrib for locations in higher latitudes
        public double[] AdjustHighLatTimes(double[] times)
        {
            double nightTime = GetTimeDifference(times[4], times[1]); // sunset to sunrise

            // Adjust Fajr
            double fajrDiff = NightPortion(methodParams[_calcMethod][0]) * nightTime;
            if (GetTimeDifference(times[0], times[1]) > fajrDiff)
                times[0] = times[1] - fajrDiff;

            // Adjust Isha
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            double ishaAngle = (methodParams[_calcMethod][3] == 0) ? methodParams

        [_calcMethod][4] : 18;
            double ishaDiff = NightPortion(ishaAngle) * nightTime;
            if (GetTimeDifference(times[4], times[6]) > ishaDiff)
                times[6] = times[4] + ishaDiff;

            // Adjust Maghrib
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            double maghribAngle = (methodParams[_calcMethod][1] == 0) ? methodParams

        [_calcMethod][2] : 4;
            double maghribDiff = NightPortion(maghribAngle) * nightTime;
            if (GetTimeDifference(times[4], times[5]) > maghribDiff)
                times[5] = times[4] + maghribDiff;

            return times;
        }

        // the night portion used for adjusting times in higher latitudes
        public double NightPortion(double angle)
        {
            double val = 0;
            if (_adjustHighLats == AngleBased)
                val = 1.0 / 60.0 * angle;
            if (_adjustHighLats == MidNight)
                val = 1.0 / 2.0;
            if (_adjustHighLats == OneSeventh)
                val = 1.0 / 7.0;

            return val;
        }

        public double[] DayPortion(double[] times)
        {
            for (int i = 0; i < times.Length; i++)
            {
                times[i] /= 24;
            }
            return times;
        }

        // compute prayer times at given julian date
        public string[] ComputeDayTimes()
        {
            double[] times = { 5, 6, 12, 13, 18, 18, 18 }; //default times

            for (int i = 0; i < numIterations; i++)
            {
                times = ComputeTimes(times);
            }

            times = AdjustTimes(times);
            return AdjustTimesFormat(times);
        }

        // adjust times in a prayer time array
        public double[] AdjustTimes(double[] times)
        {
            for (int i = 0; i < 7; i++)
            {
                times[i] += _timeZone - _lng / 15;
            }
            // ReSharper disable once PossibleLossOfFraction
            times[2] += _dhuhrMinutes / 60; //Dhuhr
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (methodParams[_calcMethod][1] == 1) // Maghrib
                times[5] = times[4] + methodParams[_calcMethod][2] / 60.0;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (methodParams[_calcMethod][3] == 1) // Isha
                times[6] = times[5] + methodParams[_calcMethod][4] / 60.0;

            if (_adjustHighLats != None)
            {
                times = AdjustHighLatTimes(times);
            }

            return times;
        }

        public string[] AdjustTimesFormat(double[] times)
        {
            string[] formatted = new string[times.Length];

            if (_timeFormat == Floating)
            {
                for (int i = 0; i < times.Length; ++i)
                {
                    formatted[i] = times[i] + "";
                }
                return formatted;
            }
            for (int i = 0; i < 7; i++)
            {
                if (_timeFormat == Time12)
                    formatted[i] = FloatToTime12(times[i], true);
                else if (_timeFormat == Time12Ns)
                    formatted[i] = FloatToTime12Ns(times[i]);
                else
                    formatted[i] = FloatToTime24(times[i]);
            }
            return formatted;
        }

        //---------------------- Misc Functions -----------------------

        // compute the difference between two times
        public double GetTimeDifference(double c1, double c2)
        {
            double diff = FixHour(c2 - c1); 
            return diff;
        }

        // add a leading 0 if necessary
        public string TwoDigitsFormat(int num)
        {
            return (num < 10) ? "0" + num : num + "";
        }

        //---------------------- Julian Date Functions -----------------------

        // calculate julian date from a calendar date
        public double JulianDate(int year, int month, int day)
        {
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }
            double a = Math.Floor(year / 100.0);
            double b = 2 - a + Math.Floor(a / 4);

            double jd = Math.Floor(365.25 * (year + 4716)) + Math.Floor(30.6001 * (month + 1)) + day + b - 1524.5;
            return jd;
        }

        //---------------------- Time-Zone Functions -----------------------

        // detect daylight saving in a given date
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public bool UseDayLightaving(int year, int month, int day)
        {
            return TimeZone.CurrentTimeZone.IsDaylightSavingTime(new DateTime(year, month, day));
        }

        // ---------------------- Trigonometric Functions -----------------------

        // degree sin
        public double Dsin(double d)
        {
            return Math.Sin(DegreeToRadian(d));
        }

        // degree cos
        public double Dcos(double d)
        {
            return Math.Cos(DegreeToRadian(d));
        }

        // degree tan
        public double Dtan(double d)
        {
            return Math.Tan(DegreeToRadian(d));
        }

        // degree arcsin
        public double Darcsin(double x)
        {
            return RadianToDegree(Math.Asin(x));
        }

        // degree arccos
        public double Darccos(double x)
        {
            return RadianToDegree(Math.Acos(x));
        }

        // degree arctan
        // ReSharper disable once IdentifierTypo
        // ReSharper disable once UnusedMember.Global
        public double Darctan(double x)
        {
            return RadianToDegree(Math.Atan(x));
        }

        // degree arctan2
        public double Darctan2(double y, double x)
        {
            return RadianToDegree(Math.Atan2(y, x));
        }

        // degree arccot
        public double Darccot(double x)
        {
            return RadianToDegree(Math.Atan(1 / x));
        }

        // Radian to Degree
        public double RadianToDegree(double radian)
        {
            return (radian * 180.0) / Math.PI;
        }

        // degree to radian
        public double DegreeToRadian(double degree)
        {
            return (degree * Math.PI) / 180.0;
        }

        public double FixAngle(double angel)
        {
            angel = angel - 360.0 * (Math.Floor(angel / 360.0));
            angel = angel < 0 ? angel + 360.0 : angel;
            return angel;
        }

        // range reduce hours to 0..23
        public double FixHour(double hour)
        {
            hour = hour - 24.0 * (Math.Floor(hour / 24.0));
            hour = hour < 0 ? hour + 24.0 : hour;
            return hour;
        }
    }
}