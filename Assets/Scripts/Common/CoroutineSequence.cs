/*
 * CoroutineSequence.cs
 * 
 * Copyright (c) 2016 Kazunori Tamura
 * This software is released under the MIT License.
 * http://opensource.org/licenses/mit-license.php
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// �R���[�`����g�ݍ��킹�Ď��s���邽�߂�Sequence�N���X
/// </summary>
public class CoroutineSequence
{
    /// <summary>
    /// Insert�Œǉ����ꂽEnumerator���Ǘ�����N���X
    /// </summary>
    private class InsertedEnumerator
    {
        /// <summary>
        /// �ʒu
        /// </summary>
        private float _atPosition;

        /// <summary>
        /// ������IEnumerator
        /// </summary>
        public IEnumerator InternalEnumerator { get; private set; }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public InsertedEnumerator(float atPosition, IEnumerator enumerator)
        {
            _atPosition = atPosition;
            InternalEnumerator = enumerator;
        }

        /// <summary>
        /// Enumerator�̎擾
        /// </summary>
        public IEnumerator GetEnumerator(Action callback)
        {
            if (_atPosition > 0f)
            {
                yield return new WaitForSeconds(_atPosition);
            }
            yield return InternalEnumerator;
            if (callback != null)
            {
                callback();
            }
        }
    }

    /// <summary>
    /// Insert���ꂽenumerator
    /// </summary>
    private List<InsertedEnumerator> _insertedEnumerators;

    /// <summary>
    /// Append���ꂽenumerator
    /// </summary>
    private List<IEnumerator> _appendedEnumerators;

    /// <summary>
    /// �I�����Ɏ��s����Action
    /// </summary>
    private Action _onCompleted;

    /// <summary>
    /// �R���[�`���̎��s��
    /// </summary>
    private MonoBehaviour _owner;

    /// <summary>
    /// �����Ŏ��s���ꂽ�R���[�`���̃��X�g
    /// </summary>
    private List<Coroutine> _coroutines;

    /// <summary>
    /// �ǉ����ꂽCoroutineSequence�̃��X�g
    /// </summary>
    private List<CoroutineSequence> _sequences;

    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    public CoroutineSequence(MonoBehaviour owner)
    {
        _owner = owner;
        _insertedEnumerators = new List<InsertedEnumerator>();
        _appendedEnumerators = new List<IEnumerator>();
        _coroutines = new List<Coroutine>();
        _sequences = new List<CoroutineSequence>();
    }

    /// <summary>
    /// enumerator��atPosition��Insert����
    /// atPosition�b���enumerator�����s�����
    /// </summary>
    public CoroutineSequence Insert(float atPosition, IEnumerator enumerator)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, enumerator));
        return this;
    }

    /// <summary>
    /// CoroutineSequence��atPosition��Insert����
    /// </summary>
    public CoroutineSequence Insert(float atPosition, CoroutineSequence sequence)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, sequence.GetEnumerator()));
        _sequences.Add(sequence);
        return this;
    }

    /// <summary>
    /// callback��atPosition��Insert����
    /// </summary>
    public CoroutineSequence InsertCallback(float atPosition, Action callback)
    {
        _insertedEnumerators.Add(new InsertedEnumerator(atPosition, GetCallbackEnumerator(callback)));
        return this;
    }

    /// <summary>
    /// enumerator��Append����
    /// Append���ꂽenumerator�́AInsert���ꂽenumerator���S�Ď��s���ꂽ��ɏ��ԂɎ��s�����
    /// </summary>
    public CoroutineSequence Append(IEnumerator enumerator)
    {
        _appendedEnumerators.Add(enumerator);
        return this;
    }

    /// <summary>
    /// CoroutineSequence��Append����
    /// </summary>
    public CoroutineSequence Append(CoroutineSequence sequence)
    {
        _appendedEnumerators.Add(sequence.GetEnumerator());
        _sequences.Add(sequence);
        return this;
    }

    /// <summary>
    /// callback��Append����
    /// </summary>
    public CoroutineSequence AppendCallback(Action callback)
    {
        _appendedEnumerators.Add(GetCallbackEnumerator(callback));
        return this;
    }

    /// <summary>
    /// �ҋ@��Append����
    /// </summary>
    public CoroutineSequence AppendInterval(float seconds)
    {
        _appendedEnumerators.Add(GetWaitForSecondsEnumerator(seconds));
        return this;
    }

    /// <summary>
    /// �I�����̏�����ǉ�����
    /// </summary>
    public CoroutineSequence OnCompleted(Action action)
    {
        _onCompleted += action;
        return this;
    }

    /// <summary>
    /// �V�[�P���X�����s����
    /// </summary>
    public Coroutine Play()
    {
        Coroutine coroutine = _owner.StartCoroutine(GetEnumerator());
        _coroutines.Add(coroutine);
        return coroutine;
    }

    /// <summary>
    /// �V�[�P���X���~�߂�
    /// </summary>
    public void Stop()
    {
        foreach (Coroutine coroutine in _coroutines)
        {
            _owner.StopCoroutine(coroutine);
        }
        foreach (InsertedEnumerator insertedEnumerator in _insertedEnumerators)
        {
            _owner.StopCoroutine(insertedEnumerator.InternalEnumerator);
        }
        foreach (IEnumerator enumerator in _appendedEnumerators)
        {
            _owner.StopCoroutine(enumerator);
        }
        foreach (CoroutineSequence sequence in _sequences)
        {
            sequence.Stop();
        }
        _coroutines.Clear();
        _insertedEnumerators.Clear();
        _appendedEnumerators.Clear();
        _sequences.Clear();
    }

    /// <summary>
    /// callback�����s����IEnumerator���擾����
    /// </summary>
    private IEnumerator GetCallbackEnumerator(Action callback)
    {
        callback();
        yield break;
    }

    /// <summary>
    /// seconds�b�ҋ@����IEnumerator���擾����
    /// </summary>
    private IEnumerator GetWaitForSecondsEnumerator(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    /// <summary>
    /// �V�[�P���X��IEnumerator���擾����
    /// </summary>
    private IEnumerator GetEnumerator()
    {
        // Insert���ꂽIEnumerator�̎��s
        int counter = _insertedEnumerators.Count;
        foreach (InsertedEnumerator insertedEnumerator in _insertedEnumerators)
        {
            Coroutine coroutine = _owner.StartCoroutine(insertedEnumerator.GetEnumerator(() =>
            {
                counter--;
            }));
            _coroutines.Add(coroutine);
        }
        // Insert���ꂽIEnumerator���S�Ď��s�����̂�҂�
        while (counter > 0)
        {
            yield return null;
        }
        // Append���ꂽIEnumerator�̎��s
        foreach (IEnumerator appendedEnumerator in _appendedEnumerators)
        {
            yield return appendedEnumerator;
        }
        // �I�����̏���
        if (_onCompleted != null)
        {
            _onCompleted();
        }
    }
}