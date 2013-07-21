= Volley Notes

http://www.youtube.com/watch?v=yhv8l9F44qo

stuff that volley does
x caching
- retry
x parallel requests
- request priority
x cancellation
- network image view
- request queue has bulk cancellation token
    mRequestQueue.cancelAll(this); // 'this' is the token
- event logging of request state transitions

- request queue instance
- JsonObjectRequest (included request type)
- image view: holds image responses, batches main thread update on interval


