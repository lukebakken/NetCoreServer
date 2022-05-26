-module(tls_client).

-export([start/0]).

start() ->
    ssl:start(),
    Options = [
        {cacertfile, "./certificates/ca.pem"},
        {certfile, "./certificates/client.pem"},
        {keyfile, "./certificates/client.key"},
        {versions, ['tlsv1.2', 'tlsv1.3']},
        {verify, verify_peer} %,
        % {secure_renegotiate, true},
        % {fail_if_no_peer_cert, true},
        % {customize_hostname_check, [
        %     {match_fun, public_key:pkix_verify_hostname_match_fun(https)}
        % ]}
    ],
    io:format("Options: ~p~n", [Options]),
    {ok, Socket} = ssl:connect("server.example.com", 2222, Options),
    io:format("Socket: ~p~n", [Socket]),
    {ok, Info} = ssl:connection_information(Socket),
    io:format("Info: ~p~n", [Info]),
    ok = ssl:close(Socket),
    init:stop().
