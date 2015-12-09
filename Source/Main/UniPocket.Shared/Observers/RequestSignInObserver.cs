using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UniPocket.Shared.Observers
{
    //<form method = "POST" action="/login_process">
    //    <h3>Log In and Authorize</h3>
    //    <div class="logingoogle-processing"></div>
    //    <a href = "#" class="btn btn-secondary login-btn-google"><span class="logo"></span><span class="text">Log In with Google</span></a><br>
    //    <a href = "/ff_signin?s=pocket&t=login" class="btn btn-secondary login-btn-firefox"><span class="logo"></span><span class="text">Log In with Firefox</span></a>
    //    <div class="signup-ordivider">or</div>
    //    <div class="loginform-formemail">
    //        <div class="form-field">
    //            <span class="error-bubble"><span class="error-msg"></span><span class="error-arrow"></span></span>
    //            <label for="feed_id">Email or username</label>
    //            <input type = "text" id="feed_id" name="feed_id" autofocus autocomplete = "off" tabindex="1" placeholder="Email or username">
    //        </div><br>
    //        <div class="form-field">
    //            <span class="error-bubble"><span class="error-msg"></span><span class="error-arrow"></span></span>
    //            <label for="login_password">Password</label>
    //            <input type = "password" id="login_password" name="password" autocomplete="off" tabindex="2" placeholder="Password">
    //        </div>
    //    </div>
    //    <input type = "hidden" class="field-form-check" name="form_check" value="e0924748d07b1ff3c7ccd8b3459a68907880b3a1141f396849ad5ba668715f8d">
    //    <input type = "hidden" name="source" value="/auth/authorize?request_token=cd952fd7-1ae6-984a-275b-445059&amp;redirect_uri=pocketapp48820%3AauthorizationFinished">
    //    <input type = "hidden" class="field-form-route" name="route" value="/auth/approve_access?request_token=cd952fd7-1ae6-984a-275b-445059&from_login=1&permission=amd&approve_flag=1&redirect_uri=pocketapp48820%3AauthorizationFinished">
    //    <div class="login-processing"></div>
    //    <div class="loggedout-ctas">
    //        <div class="btn btn-secondary btn-small btn-deny">No, thanks</div>
    //        <input type = "submit" value="Authorize" class="btn btn-important btn-small login-btn-email btn-authorize">
    //    </div>
    //    <p><a class="forgot" href="/forgot">Forgot your username or password</a> <span class="rarrow">&rsaquo;</span></p>
    //</form>

    public class RequestSignInObserver : HttpContentObserverBase<string>
    {
        private const string HiddenInputFormCheck = "form_check";
        private const string HiddenInputSource = "source";
        private const string HiddenInputRoute = "route";
        private const string HiddenInputSsoSource = "sso_source";
        private const string HiddenInputRequestToken = "request_token";
        private const string HiddenInputLocaleLanguage = "locale_lang";
        private const string HiddenInputSourcePage = "source_page";

        public event Action<IDictionary<string, string>> ParseSignInParametersCompleted;

        public override void OnCompleted()
        {
            HtmlDocument dom = new HtmlDocument();
            dom.LoadHtml(this.HtmlContent);

            // Parses the hidden inputs for authorization request parameters
            var inputs = from input in dom.DocumentNode.Descendants("input")
                         where input.Attributes["type"] != null && input.Attributes["type"].Value == "hidden"
                         select new
                         {
                             Key = input.Attributes["name"].Value,
                             Value = input.Attributes["value"].Value
                         };

            ParseSignInParametersCompleted?.Invoke(inputs.ToDictionary(i => i.Key, i => i.Value));
        }
    }
}